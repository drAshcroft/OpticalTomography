
for I=1:size(sinoGram,2)
    Projections{I}=sinoGram(:,I)';
end

nProjections = length(Projections);

angleStep=360.0/nProjections;

sizeProjections = length(Projections{1});

reconImage = zeros([sizeProjections sizeProjections]);

clear filter;
B=0:(sizeProjections/2);
filter(1:length(B)) =B;
B=(sizeProjections/2):-1:1;
filter(length(filter):length(filter)+length(B)-1) =B;
filter=filter./(length(filter)/2)^2*3.14;

addition = zeros([sizeProjections sizeProjections]);
for I=1:nProjections
    projection = Projections{I};
    
    fProjection =real(ifft(  fft(projection) .* filter ));
    
    
    for J=1:sizeProjections
       addition(:,J)= fProjection(J);
    end
    
    addition=imrotate(addition,angleStep*I,'bilinear','crop');
    
    reconImage=reconImage + addition;
end

figure;imagesc(reconImage);colormap gray;
figure;imagesc(D);colormap gray;

double(max(D(:)))/max(reconImage(:))